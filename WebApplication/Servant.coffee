# Server side
if !document?
  actionInProgress = null

  self.status = ( message ) ->
    self.postMessage( { functionName: actionInProgress, status: message } )
  
  serve = ( command ) ->
    if self[ command.action ]?
      actionInProgress = command.action
      result = self[ command.action ]( command.input )
      actionInProgress = null
      if command.action isnt "eval"
        self.postMessage( { functionName: command.action, result: result } )

  self.addEventListener(
    "message",
    ( e ) -> serve( e.data ),
    false )

# Client side
if document?
  nameOfFunction = (f) ->
    string = f.toString()
    string = string.substr( "function ".length )
    return string.substr( 0, string.indexOf( "(" ) )
  
  ensureThatFunctionIsNamed = ( functionObject ) ->
    unless functionObject.name?
      functionObject.name = nameOfFunction( functionObject )
    return

  class @Servant
    constructor: ( @scripts... ) ->
      @functions = new Object()
      @functionStatusHandlers = new Object()
      @functionResultHandlers = new Object()
      @constructWorker()     
        
      @load( script ) for script in @scripts
      return
      
    evalInWorker: ( string ) ->
      @worker.postMessage( { action: "eval", input: string } )
      return

    load: ( script ) ->
      @worker.postMessage( { action: "importScripts", input: script } )
      return
      
    constructWorker: () ->
      @worker = new Worker( "Servant.js" )
      @worker.onerror = ( event ) => alert( event.filename + ", line " + event.lineno + ": " + event.message )
      @worker.onmessage = ( event ) =>
        if event.data.status? and event.data.functionName?
          this[ event.data.functionName ].status = event.data.status
          if @functionStatusHandlers[ event.data.functionName ]?
            @functionStatusHandlers[ event.data.functionName ]( event.data.status )

        if event.data.result? and event.data.functionName?
          this[ event.data.functionName ].result = event.data.result
          if @functionResultHandlers[ event.data.functionName ]?
            @functionResultHandlers[ event.data.functionName ]( event.data.result )
          
    addFunction: ( functionObject, resultHandler, statusHandler ) ->

      ensureThatFunctionIsNamed( functionObject )
      if !functionObject.name? or functionObject.name.length == 0
        throw new Error( "anonymous function passed to Servant.addFunction" )
        
      @functions[ functionObject.name ] = functionObject
      
      @evalInWorker( functionObject.toString() )
      
      if resultHandler?
        if typeof( resultHandler ) isnt "function"
          throw new Error( "type of resultHandler isn't function." )
        @functionResultHandlers[ functionObject.name ] = resultHandler
        
      if statusHandler?
        if typeof( statusHandler ) isnt "function"
          throw new Error( "type of statusHandler isn't function." )
        @functionStatusHandlers[ functionObject.name ] = statusHandler
        
      this[ functionObject.name ] = ( argument ) ->
        @worker.postMessage( { action: functionObject.name, input: argument } )
        
    cancelAllWork: () ->
      @worker.terminate()
      @constructWorker()
      
      @load( script ) for script in @scripts
      
      for own name, functionObject of @functions
        @evalInWorker( functionObject.toString() )
      