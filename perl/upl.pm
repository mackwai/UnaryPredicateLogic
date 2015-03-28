use strict;
use IPC::Open2;

sub decide;
sub complexity;
sub query;

my $out;
my $in;
my $pid = 0;


sub query
{
  $pid = open2( $out, $in, 'upl --query' ) unless $pid;
  (my $statement, my $query_type) = @_;
  $statement = $_ if not defined $statement;
  $query_type = '' if not defined $query_type;
  print $in $statement;
  print $in "\n$query_type?\n";
  my $result = <$out>;
  die "failed to get result from upl.exe" unless defined $result;
  $result =~ s/[\r\n]//g;
  return $result;
}

sub decide
{
  return &query;
}

sub complexity
{
  return int query( @_ ? $_[0] : $_, 'c' );
}

sub is_sentence
{
  my $result = query( @_ ? $_[0] : $_, 's' );
  return $result =~ /Valid/;
}

END
{
  if ( $pid )
  {
    close $in;
    close $out;
    kill -9, $pid;
    waitpid( $pid, 0 );
  }
}

return 1;