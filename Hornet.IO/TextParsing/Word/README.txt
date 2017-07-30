Contents of this directory can largely be ignored and treated as an external lib.

The code for binary word files isn't in a Nuget package so is embedded within
this solution.  It reads text based on the old Word character position system
and is the code taken from here:
http://www.codeproject.com/Articles/22738/Read-Document-Text-Directly-from-Microsoft-Word-Fi
Except I have modified the TextLoader class to use stringbuilder rather than
concatenation for performance, see comments within that class.
