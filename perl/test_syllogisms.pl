# somerby.net/mack/logic
# Copyright (C) 2016 MacKenzie Cumings
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
use upl;

sub operand;
sub operand { @_ ? $_[0] : $_ }

sub false;
sub false { "" }
sub true { 1 }

sub print_if_necessary;
sub print_if_necessary
{
  for (&operand)
  {
    my $result = decide; #upl;
    say $_ if $result eq "Necessary";
    #say $_ if $result eq "Necessary\r\n";
  }
}

sub join_with;
sub join_with
{
  (my $connective, my @matrices) = @_;
  my $connected = join ")$connective(", @matrices;
  return "($connected)";
}

sub equivalent;
sub equivalent
{
  (my $statement1, my $statement2) = @_;
  return decide( join_with "<=>", $statement1, $statement2 ) eq "Necessary";
}

sub inconsistent;
sub inconsistent
{
  return decide( join_with "&", @_ ) eq "Impossible";
}

sub necessary;
sub necessary
{
  return decide( join_with "&", @_ ) eq "Necessary";
}

sub proved_by;
sub proved_by
{
  (my $conclusion, my @premises) = @_;
  return false if inconsistent @premises;
  return false if necessary $conclusion;
  my $premises = join_with "&", @premises;
  return necessary "$premises->($conclusion)";
}


for my $modernity ( '', "'" )
{
  say "";
  if ( $modernity )
  {
    say "Modern interpretation (O has existential import, A does not)";
  }
  else
  {
    say "Parsons' interpretation (A has existential import, O does not)";
  }
  for my $figure (
    "(M1P$modernity&S2M$modernity)->S3P$modernity",
    "(P1M$modernity&S2M$modernity)->S3P$modernity",
    "(M1P$modernity&M2S$modernity)->S3P$modernity",
    "(P1M$modernity&M2S$modernity)->S3P$modernity" )
  {
    $figure =~/\(([MP]).*&([SM])/;
    my $subject_term1 = $1;
    my $subject_term2 = $2;
    for my $form1 ( 'a', 'e', 'i', 'o' )#, 'u', 'y' )
    {
      my $substitution1 = $figure;
      $substitution1 =~ s/1/$form1/;
      for my $form2 ( 'a', 'e', 'i', 'o' )#, 'u', 'y' )
      {
        my $substitution2 = $substitution1;
        $substitution2 =~ s/2/$form2/;
        for my $form3 ( 'a', 'e', 'i', 'o' )#, 'u', 'y' )
        {
          my $substitution3 = $substitution2;
          $substitution3 =~ s/3/$form3/;
          say $substitution3 if necessary $substitution3;
        }
      }
    }
  }
}

say "";
say "Assume a priori that the subject terms are not empty...";

for my $figure (
  "(M1P'&S2M')->S3P'",
  "(P1M'&S2M')->S3P'",
  "(M1P'&M2S')->S3P'",
  "(P1M'&M2S')->S3P'" )
{
  $figure =~/\(([MP]).*&([SM])/;
  my $subject_term1 = $1;
  my $subject_term2 = $2;
  for my $form1 ( 'a', 'e', 'i', 'o' )#, 'u', 'y' )
  {
    my $substitution1 = $figure;
    $substitution1 =~ s/1/$form1/;
    for my $form2 ( 'a', 'e', 'i', 'o' )#, 'u', 'y' )
    {
      my $substitution2 = $substitution1;
      $substitution2 =~ s/2/$form2/;
      for my $form3 ( 'a', 'e', 'i', 'o' )#, 'u', 'y' )
      {
        my $substitution3 = $substitution2;
        $substitution3 =~ s/3/$form3/;
        my $argument_with_a_priori = "((3x,$subject_term1"
          ."x)&(3x,$subject_term2"
          ."x))->($substitution3)";
        say $argument_with_a_priori if necessary $argument_with_a_priori;
      }
    }
  }
}

say "";
say "Assume a priori that no terms are empty...";

for my $figure (
  "(M1P'&S2M')->S3P'",
  "(P1M'&S2M')->S3P'",
  "(M1P'&M2S')->S3P'",
  "(P1M'&M2S')->S3P'" )
{
  $figure =~/\(([MP]).*&([SM])/;
  my $subject_term1 = $1;
  my $subject_term2 = $2;
  for my $form1 ( 'a', 'e', 'i', 'o' )#, 'u', 'y' )
  {
    my $substitution1 = $figure;
    $substitution1 =~ s/1/$form1/;
    for my $form2 ( 'a', 'e', 'i', 'o' )#, 'u', 'y' )
    {
      my $substitution2 = $substitution1;
      $substitution2 =~ s/2/$form2/;
      for my $form3 ( 'a', 'e', 'i', 'o' )#, 'u', 'y' )
      {
        my $substitution3 = $substitution2;
        $substitution3 =~ s/3/$form3/;
        my $argument_with_a_priori = "((3x,Mx)&(3x,Px)&(3x,Sx))->($substitution3)";
        say $argument_with_a_priori if necessary $argument_with_a_priori;
      }
    }
  }
}

say "";
say "Assume a priori that S and P are not empty...";

for my $figure (
  "(M1P'&S2M')->S3P'",
  "(P1M'&S2M')->S3P'",
  "(M1P'&M2S')->S3P'",
  "(P1M'&M2S')->S3P'" )
{
  $figure =~/\(([MP]).*&([SM])/;
  my $subject_term1 = $1;
  my $subject_term2 = $2;
  for my $form1 ( 'a', 'e', 'i', 'o' )#, 'u', 'y' )
  {
    my $substitution1 = $figure;
    $substitution1 =~ s/1/$form1/;
    for my $form2 ( 'a', 'e', 'i', 'o' )#, 'u', 'y' )
    {
      my $substitution2 = $substitution1;
      $substitution2 =~ s/2/$form2/;
      for my $form3 ( 'a', 'e', 'i', 'o' )#, 'u', 'y' )
      {
        my $substitution3 = $substitution2;
        $substitution3 =~ s/3/$form3/;
        my $argument_with_a_priori = "((3x,Px)&(3x,Sx))->($substitution3)";
        say $argument_with_a_priori if necessary $argument_with_a_priori;
      }
    }
  }
}


