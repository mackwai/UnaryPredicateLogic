# somerby.net/mack/logic
# Copyright (C) 2014 MacKenzie Cumings
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


my $expected_results = 'The argument in Socrates.txt is valid.
File not found: DiSpezio120.txt
The argument in DiSpezio135.txt is valid.
The argument in Hurley213.txt is valid.
The argument in Hurley454.txt is invalid.
The argument in Hurley457.txt is invalid.
The contents of ModalTest1.txt are self-contradictory.
The contents of ModalTest2.txt are necessarily true.
The argument in ModalTest4.txt is valid.
The argument in SocratesOverload.txt is valid.
The contents of EmptyUniverseTest.txt are self-contradictory.
The contents of SimpleContradiction.txt are self-contradictory.
The contents of NullPredicates.txt are possible.
The argument in MixedNullUnaryPredicates1.txt is valid.
The contents of BigNullPredicates.txt are necessarily true.
invalid: Logic.ParseError: Could not parse "(~A&B&C&D&E)(~A&B&C&D&~E)"
The contents of RulesOfLogic.txt are necessarily true.
The contents of ToBeOrNotToBe.txt are necessarily true.
';

my $actual_results = `..\\upl.exe Socrates.txt DiSpezio120.txt DiSpezio135.txt Hurley213.txt Hurley454.txt Hurley457.txt ModalTest1.txt ModalTest2.txt ModalTest4.txt SocratesOverload.txt EmptyUniverseTest.txt SimpleContradiction.txt NullPredicates.txt MixedNullUnaryPredicates1.txt BigNullPredicates.txt InvalidNullPredicates.txt RulesOfLogic.txt ToBeOrNotToBe.txt`;

print $actual_results;
print "\n";

print ( $expected_results eq $actual_results ? "pass" : "fail" );
print "\n";
system "pause";
