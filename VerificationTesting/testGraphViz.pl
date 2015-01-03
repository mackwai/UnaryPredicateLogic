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

use strict;
use feature "say";

my $input = @ARGV ? $ARGV[0] : "Socrates.txt";
#say $input;
my $dot = $input;
$dot =~ s/(\.[^.]+)$/.dot/;
#say $dot;
$dot .= "$dot.dot" if $dot eq $input;
#say $dot;
my $png = $dot;
$png =~ s/dot$/png/;
#say $png;

system "..\\upl -g $input > $dot";
#die if
system "dot -Tpng -o$png $dot";
# != 0;
system "$png";