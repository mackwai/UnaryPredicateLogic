# somerby.net/mack/logic
# Copyright (C) 2015 MacKenzie Cumings
#
# This program is free software; you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation; either version 2 of the License, or
# (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License along
# with this program; if not, write to the Free Software Foundation, Inc.,
# 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

open FILE, '> expected_results.txt';

print FILE `..\\mpl.exe Socrates.txt DiSpezio120.txt DiSpezio135.txt Hurley213.txt Hurley454.txt Hurley457.txt ModalTest1.txt ModalTest2.txt ModalTest4.txt SocratesOverload.txt EmptyUniverseTest.txt NullPredicates.txt MixedNullUnaryPredicates1.txt BigNullPredicates.txt InvalidNullPredicates.txt RulesOfLogic.txt ToBeOrNotToBe.txt`;

close FILE;
