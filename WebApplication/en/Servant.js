// Generated by CoffeeScript 1.10.0
(function() {
  var actionInProgress, ensureThatFunctionIsNamed, nameOfFunction, serve,
    slice = [].slice,
    hasProp = {}.hasOwnProperty;

  if (true) {
    if (typeof document === "undefined" || document === null) {
      actionInProgress = null;
      self.status = function(message) {
        return self.postMessage({
          functionName: actionInProgress,
          status: message
        });
      };
      serve = function(command) {
        var result;
        if (self[command.action] != null) {
          actionInProgress = command.action;
          result = self[command.action](command.input);
          actionInProgress = null;
          if (command.action !== "eval") {
            return self.postMessage({
              functionName: command.action,
              result: result
            });
          }
        }
      };
      self.addEventListener("message", function(e) {
        return serve(e.data);
      }, false);
    }
    if (typeof document !== "undefined" && document !== null) {
      nameOfFunction = function(f) {
        var string;
        string = f.toString();
        string = string.substr("function ".length);
        return string.substr(0, string.indexOf("("));
      };
      ensureThatFunctionIsNamed = function(functionObject) {
        if (functionObject.name == null) {
          functionObject.name = nameOfFunction(functionObject);
        }
      };
      this.Servant = (function() {
        function Servant() {
          var i, len, ref, script, scripts;
          scripts = 1 <= arguments.length ? slice.call(arguments, 0) : [];
          this.scripts = scripts;
          this.functions = new Object();
          this.functionStatusHandlers = new Object();
          this.functionResultHandlers = new Object();
          this.constructWorker();
          ref = this.scripts;
          var UrlExists = function (url)
          {
              var http = new XMLHttpRequest();
              http.open('HEAD', url, false);
              http.send();
              return http.status!=404;
          };
          for (i = 0, len = ref.length; i < len; i++) {
            script = ref[i];
            if ( !UrlExists(script) )
              alert( script + " not found!" );
            this.load(script);
          }
          return;
        }

        Servant.prototype.evalInWorker = function(string) {
          this.worker.postMessage({
            action: "eval",
            input: string
          });
        };

        Servant.prototype.load = function(script) {
          this.worker.postMessage({
            action: "importScripts",
            input: script
          });
        };

        Servant.prototype.constructWorker = function() {
          var UrlExists = function (url)
          {
              var http = new XMLHttpRequest();
              http.open('HEAD', url, false);
              http.send();
              return http.status!=404;
          };
          if ( !UrlExists("Servant.js") )
            alert( "Servant.js" + " not found!" );
          this.worker = new Worker("Servant.js");
          this.worker.onerror = (function(_this) {
            return function(event) {
              return alert(event.filename + ", line " + event.lineno + ": " + event.message);
            };
          })(this);
          return this.worker.onmessage = (function(_this) {
            return function(event) {
              if ((event.data.status != null) && (event.data.functionName != null)) {
                _this[event.data.functionName].status = event.data.status;
                if (_this.functionStatusHandlers[event.data.functionName] != null) {
                  _this.functionStatusHandlers[event.data.functionName](event.data.status);
                }
              }
              if ((event.data.result != null) && (event.data.functionName != null)) {
                _this[event.data.functionName].result = event.data.result;
                if (_this.functionResultHandlers[event.data.functionName] != null) {
                  return _this.functionResultHandlers[event.data.functionName](event.data.result);
                }
              }
            };
          })(this);
        };

        Servant.prototype.addFunction = function(functionObject, resultHandler, statusHandler) {
          ensureThatFunctionIsNamed(functionObject);
          if ((functionObject.name == null) || functionObject.name.length === 0) {
            throw new Error("anonymous function passed to Servant.addFunction");
          }
          this.functions[functionObject.name] = functionObject;
          this.evalInWorker(functionObject.toString());
          if (resultHandler != null) {
            if (typeof resultHandler !== "function") {
              throw new Error("type of resultHandler isn't function.");
            }
            this.functionResultHandlers[functionObject.name] = resultHandler;
          }
          if (statusHandler != null) {
            if (typeof statusHandler !== "function") {
              throw new Error("type of statusHandler isn't function.");
            }
            this.functionStatusHandlers[functionObject.name] = statusHandler;
          }
          return this[functionObject.name] = function(argument) {
            return this.worker.postMessage({
              action: functionObject.name,
              input: argument
            });
          };
        };

        Servant.prototype.cancelAllWork = function() {
          var functionObject, i, len, name, ref, ref1, results, script;
          this.worker.terminate();
          this.constructWorker();
          ref = this.scripts;
          for (i = 0, len = ref.length; i < len; i++) {
            script = ref[i];
            this.load(script);
          }
          ref1 = this.functions;
          results = [];
          for (name in ref1) {
            if (!hasProp.call(ref1, name)) continue;
            functionObject = ref1[name];
            results.push(this.evalInWorker(functionObject.toString()));
          }
          return results;
        };

        return Servant;

      })();
    }
  } else {
    window.importScripts = function() {
      var i, len, request, scriptElement, src;
      for (i = 0, len = arguments.length; i < len; i++) {
        src = arguments[i];
        request = new XMLHttpRequest();
        request.open("GET", src, false);
        request.send("");
        scriptElement = document.createElement("script");
        scriptElement.type = "text/javascript";
        scriptElement.text = request.responseText;
        document.getElementsByTagName("head")[0].appendChild(scriptElement);
      }
    };
    nameOfFunction = function(f) {
      var string;
      string = f.toString();
      string = string.substr("function ".length);
      return string.substr(0, string.indexOf("("));
    };
    ensureThatFunctionIsNamed = function(functionObject) {
      if (functionObject.name == null) {
        functionObject.name = nameOfFunction(functionObject);
      }
    };
    this.Servant = (function() {
      function Servant() {
        var i, len, ref, script, scripts;
        scripts = 1 <= arguments.length ? slice.call(arguments, 0) : [];
        this.scripts = scripts;
        ref = this.scripts;
        for (i = 0, len = ref.length; i < len; i++) {
          script = ref[i];
          this.load(script);
        }
        return;
      }

      Servant.prototype.load = function(src) {
        var head, script;
        head = document.getElementsByTagName("head")[0];
        script = document.createElement("script");
        script.type = "text/javascript";
        script.src = src;
        head.appendChild(script);
      };

      Servant.prototype.addFunction = function(functionObject, resultHandler, statusHandler) {
        ensureThatFunctionIsNamed(functionObject);
        if ((functionObject.name == null) || functionObject.name.length === 0) {
          throw new Error("anonymous function passed to Servant.addFunction");
        }
        this[functionObject.name] = resultHandler != null ? function(argument) {
          return resultHandler(functionObject(argument));
        } : function(argument) {
          return functionObject(argument);
        };
      };

      Servant.prototype.cancelAllWork = function() {};

      return Servant;

    })();
  }

}).call(this);
