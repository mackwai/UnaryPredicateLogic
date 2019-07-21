// Truth Table Generator
// 
// This program generates a truth table given a well formed input string. The characters 
// used in the html outputs (to represent the connectives and truth 
// values) are set in the htmlchar() function immediately 
// below.
// 
// Copyright (C) 2014  MacKenzie Cumings
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
/*************************************************************************************/

function htmlchar(c) {
	switch(c) {
		case true : return 'T';
		case false : return '&perp;';
		case '~' : return '&not;';
		case '&' : return '<b>&middot;</b>';
		case '|' : return '&or;';
		case '->' : return '&rarr;';
		case '<=>' : return '&equiv;';
		case '^' : return '^';
    case '!' : return '&#65516;';
		default : return c;
	}	
}

function ThrowErrorInTruthTableGenerator()
{
  throw "Error in truth table generator.";
}

/*************************************************************************************/

// main construction function
function construct(input) {
	var time = new Date().getTime();
	var formulas = input.replace(/ /g,'');// remove whitespace
	if(formulas=='') {ThrowErrorInTruthTableGenerator();};
	var r = badchar(formulas);
	if(r>=0) {throw "The string you entered contains the following unrecognized symbol: "+formulas[r];};
		
	formulas = formulas.split(','); // create an array of formulas
	var trees = formulas.map(parse); // create an array of parse trees
	for(var i=0;i<trees.length;i++) { // adds outermost parentheses if needed
		if(trees[i].length==0) {
			formulas[i] = '('+formulas[i]+')';
			trees[i] = parse(formulas[i]);
		}
	}
	if(trees.filter(function(a) {return a.length==0;}).length>0) { // checks if any formulas are still malformed
		ThrowErrorInTruthTableGenerator();
	}
	
	var table = mkTable(formulas,trees);

  return htmlTable(table,trees,false);
}

// (Table,[Tree],Boolean) -> String
// Takes a table (as output by mkTable), the trees it's a table of, and a boolean and
// returns an HTML table. If the boolean is set to true, it only prints the column 
// under the main connective.
function htmlTable(table,trees,flag) {
	var rownum = table[0].length;
	var mcs = []; // indices of the main connectives
	for(var i=0;i<trees.length;i++) {
		mcs.push(mcindex(trees[i]))
	}
	var out = '<table class="truth">'; // start the html table
	out += mkTHrow(table); // make the top th row
	for(var i=1;i<table[0].length;i++) { // make the td rows below
		out += mkTDrow(table,i);
	}
	return out+'</table>'; // return the html table
	
	function mkTHrow(tbl) {
		var rw = '<tr>';
		for(var i=0;i<tbl.length;i++) { // i = table segment
			for(var j=0;j<tbl[i][0].length;j++) { // row = 0, j = cell
				if(j==tbl[i][0].length-1 && i!=tbl.length-1) {
					rw += '<th>'+htmlchar(tbl[i][0][j])+'</th>'+'<th class="dv"></th><th></th>';
				} else {
					rw += '<th>'+htmlchar(tbl[i][0][j])+'</th>';
				}
			}
		}
		return rw+'</tr>';
	}
	
	function mkTDrow(tbl,r) {
		var rw = '<tr>';
		for(var i=0;i<tbl.length;i++) { // i = table segment
			for(var j=0;j<tbl[i][r].length;j++) { // r = row, j = cell
				if(mcs[i-1]==j) {
					rw += '<td class="mc">'+htmlchar(tbl[i][r][j])+'</td>';
				} else if(flag && i>0) {
					rw += '<td></td>'
				} else {
					rw += '<td>'+htmlchar(tbl[i][r][j])+'</td>';
				}
				if(j==tbl[i][r].length-1 && i!=tbl.length-1) {
					rw += '<td class="dv"></td><td></td>'
				}
			}
		}
		return rw+'</tr>';
	}
}

// Tree -> Int
// Finds the index of the main connective in the tree
function mcindex(t) {
	if(t.length == 2 || t.length==1) {
		return 0;
	} else {
		return countleaves(t[1])+1;
	}
}

// Tree -> Int
// Takes a tree and returns the number of leaves (terminal nodes) in the tree
function countleaves(t) {
	var out = 0;
	for(var i=0;i<t.length;i++) {
		if(t[i] instanceof Array) {
			out += countleaves(t[i]);
		} else {out += 1;}
	}
	return out;
}

// ([String],[Tree]) -> Table
// Takes an array of formulas and their parse trees and returns a truth table as a 
// multidimensional array.  For n formulas, the array contains n+1 elements.  The first
// element is the lhs of the table, and the succeeding elements are the table segments
// for each passed formula.
function mkTable(fs,ts) {
	var lhs = mklhs(fs);
	var rhs = [];
	for(var i=0;i<fs.length;i++) {
		rhs.push(mktseg(fs[i],ts[i],lhs));
	}
	return [lhs].concat(rhs);
}

// [String] -> LHSTable
// Takes an array of strings and makes the left hand side of a table (i.e. the rows
// with all the tv assignments)
function mklhs(fs) {
	var atomic = [];
	var tvrows = [];
	for(var i=0;i<fs.length;i++) {
		atomic = atomic.concat(getatomic(fs[i]))
	}
	atomic = sorted(rmDup(atomic));
	if(atomic.indexOf('#')>=0) {
		tvrows = tvcomb(atomic.length-1);
		tvrows = tvrows.map(function(x) {return [false].concat(x);});
	} else {tvrows = tvcomb(atomic.length);}
	return [atomic].concat(tvrows);
}

// (String, Tree, LHSTable) -> TableSegment
// Takes a tree, the formula it's a tree of, and a LHSTable, and returns a TableSegment 
// for the formula 
function mktseg(f,t,lhs) {
	var tbrows=[];
	for(var i=1;i<lhs.length;i++) {
		var a = mkAss(lhs[0],lhs[i]);
		var row = evlTree(t,a);
		row = flatten(row);
		tbrows.push(row);
	}
	tbrows = [flatten(t)].concat(tbrows);
	return tbrows;
}

// String -> [Char]
// Takes a wff and returns an array with all the atomic sentences in the wff.  The 
// array has duplicates removed and is sorted in alphabetical order.
function getatomic(s) {
	var out = [];
	for(var i=0;i<s.length;i++) {
		if(isA(s[i])) {out.push(s[i]);}
	}
	return out;
}

// Int -> [[Bool]]
// Takes an int n and returns 2^n truth value assignments (each an array)
function tvcomb(n) {
	if(n==0) {return [[]];}
	var prev = tvcomb(n-1);
	var mt = function(x) {return [true].concat(x);};
	var mf = function(x) {return [false].concat(x);};
	return prev.map(mt).concat(prev.map(mf));
}

// ([Char], [Bool]) -> Assignment
// Takes an array of n Chars and an array of n Bools and returns a assignment that
// assigns the nth Bool to the nth Char.
function mkAss(s,b) {
	var a = new Object();
	for(var i=0;i<s.length;i++) {
		a[s[i]] = b[i];
	}
	return a;
}

// Tree -> Array 
// Takes an evaluated tree and turns it into a one dimensional array
function flatten(t) {
	if(t.length==5) {
		return [].concat(t[0]).concat(flatten(t[1])).concat(t[2]).concat(flatten(t[3])).concat(t[4]);
	} else if(t.length==2) {
		return [].concat(t[0]).concat(flatten(t[1]));
	} else if(t.length==1) {
		return [].concat(t[0]);
	}
}

// (Tree,Assignment) -> Tree
// Takes a tree and an assignment of booleans to atomic sentences and returns an 
// evaluated tree (i.e. with all atomic sentences and connectives replaced by booleans).
function evlTree(t,a) {
	if(t.length==5) {
		var t1 = evlTree(t[1],a);
		var t3 = evlTree(t[3],a);
		return ['',t1,gtTv([t[2],t1,t3]),t3,'']
	} else if(t.length==2) {
		var t1 = evlTree(t[1],a);
		return [gtTv([t[0],t1]),t1];
	} else if(t.length==1) {
		return [a[t[0]]];
	}
}

// Array -> Boolean
// Takes an array, the first element of which is a connective, and the rest of which
// are evaluated trees of the formulas it connects, and returns the truth value 
// associated with the connective
function gtTv(arr) {
	switch(arr[0]) {
		case '~' : return !tv(arr[1]);
		case '&' : return tv(arr[1])&&tv(arr[2]);
		case '|' : return tv(arr[1])||tv(arr[2]);
		case '->' : return (!tv(arr[1])||tv(arr[2]));
		case '<=>' : return (tv(arr[1])==tv(arr[2]));
		case '^' : return (! (tv(arr[1])==tv(arr[2])) );
    case '!' : return (! (tv(arr[1])||tv(arr[2])) );
	}
	function tv(x) {
		switch(x.length) {
			case 5 : return x[2];
			case 2 : return x[0];
			case 1 : return x[0];
		}
	}
}

// Remove duplicates from an array
function rmDup(a) {
	return a.filter(function(el,pos) {return a.indexOf(el)==pos;});
}

// [Char] -> [Char]
// Takes an array of chars and returns the array sorted from smallest to largest
function sorted(a) {
	var b = a.map(function(x) {return x.charCodeAt(0);});
	b = b.sort(function(b,c) {return b-c;});
	return b.map(function(x) {return String.fromCharCode(x);});
}


// FORMULA PARSING CODE
//=====================

// String -> Tree
// Takes a string and if it's a wff, returns a parse tree of the string, otherwise
// returns an empty array.
function parse(s) {
	if(s.length==0) {return [];}
	var s1 = [];
	var s2 = [];
	if(isU(s[0])) {
		s1 = parse(s.substring(1));
		return s1.length ? [s[0],s1] : [];
	}
	if(s[0] =='(' && s[s.length-1]==')') {
		var a = gSub(s);
		if(a.indexOf(undefined)>=0 || a.indexOf('')>=0) {
			return [];
		} else {
			s1 = parse(a[0]);
			s2 = parse(a[2]);
			if(s1.length && s2.length) {
				return ['(',s1,a[1],s2,')'];
			} else {return [];}
		}
	}
	else {return isA(s) ? [s] : []}
}

// String -> Bool
// Determines if s is an atomic wff
function isA(s) {
	if(s.length!=1) {return false;}
	var pr = '#ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuwxyz';
	return pr.indexOf(s)>=0;
}


// String -> Bool
// Determines if s begins with a unary connective
function isU(s) {
	return s.indexOf('~')==0;
}

// String -> [String]
// takes a string beginning with '(' and ending with ')', and determines if there is a
// binary connective enclosed only by the outermost parentheses.  If so, returns an array
// with the string to the left and the string to the right of the binary connective; 
// otherwise returns an array of three undefined's.
function gSub(s) {
	var stk = [];
	var l = 0;
	for(var i=0;i<s.length;i++) {
		if(s[i]=='(') {
			stk.push('(');
		} else if(s[i]==')' && stk.length>0) {
			stk.pop();
		} else if(stk.length==1 && (l = isB(s.substring(i)))>0) {
			return [s.substring(1,i),s.substring(i,i+l),s.substring(i+l,s.length-1)];
		}	
	}
	return [undefined,undefined,undefined];
}

// String -> Int
// takes a string and determines if it begins with a binary connective.  If so, returns
// the length of the connective, otherwise returns 0.
function isB(s) {
	var bc = ['&','|','->','<=>','^','!'];
	for(var i=0;i<bc.length;i++) {
		if(s.indexOf(bc[i]) == 0) {
			return bc[i].length;
		}
	}
	return 0;
}

// String -> Int
// Checks if the string contains any inadmissible characters
function badchar(s) {
	var x = ',()~=&<>-|!#ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuwxyz';
	for(var i=0;i<s.length;i++) {
		if(x.indexOf(s[i])<0) {
			return i;
		}
	}
	return -1;
}