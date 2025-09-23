#!/bin/bash


shopt -s nullglob

#f_data="./test/"
f_data="./data/"

for f in $(find $f_data -name '*.wav')
do

f_base=`basename $f .wav`
echo $f_base

audiowaveform -i "$f" -o "$f_data$f_base.dat" -z 2 -w 1600 -h 500
audiowaveform -i "$f_data$f_base.dat" -o "$f_data$f_base.png" -z auto -w 1600 -h 500

rm "$f_data$f_base.dat"

done
