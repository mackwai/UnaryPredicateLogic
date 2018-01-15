REM  somerby.net/mack/logic
REM  Copyright (C) 2015 MacKenzie Cumings
REM 
REM  This program is free software; you can redistribute it and/or modify
REM  it under the terms of the GNU General Public License as published by
REM  the Free Software Foundation; either version 2 of the License, or
REM  (at your option) any later version.
REM 
REM  This program is distributed in the hope that it will be useful,
REM  but WITHOUT ANY WARRANTY; without even the implied warranty of
REM  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
REM  GNU General Public License for more details.
REM 
REM  You should have received a copy of the GNU General Public License along
REM  with this program; if not, write to the Free Software Foundation, Inc.,
REM  51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

cd WebApplication
del "Notepad++Plugin.zip"
mkdir Plugin\Config
copy "..\Notepad++Plugin\bin\Release\Notepad++Plugin.dll" .\Plugin\SymbolicLogic.dll
copy "C:\Program Files (x86)\Notepad++\plugins\Config\Decide.bmp" .\Plugin\Config\Decide.bmp
cd Plugin
"C:\Program Files\7-Zip\7z.exe" u "..\Notepad++Plugin.zip" SymbolicLogic.dll Config
cd ..
rmdir /Q /S Plugin
cd ..
