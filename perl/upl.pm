use strict;
use IPC::Open2;

sub decide;

my $out;
my $in;
my $pid = 0;

sub decide
{
  $pid = open2( $out, $in, 'upl --query' ) unless $pid;
  print $in ( @_ ? $_[0] : $_ );
  print $in "\n?\n";
  my $result = <$out>;
  die unless defined $result;
  $result =~ s/[\r\n]//g;
  return $result;
}

END
{
  close $in;
  close $out;
  kill -9, $pid;
  waitpid( $pid, 0 );
}

return 1;