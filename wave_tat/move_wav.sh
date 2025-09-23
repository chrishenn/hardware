#!/bin/bash


shopt -s nullglob


for f in $(find ~/Downloads/data/wav/ -name '*.wav' )

do

fname=`tr -dc A-Za-z0-9 </dev/urandom | head -c 20`

mv "$f" "/home/chris/Documents/wave-tat/data/$fname.wav"

done
