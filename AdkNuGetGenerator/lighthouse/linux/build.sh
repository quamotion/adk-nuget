#!/bin/bash
gcc -shared -o liblighthouse-x86.so -fPIC -ldl lighthouse.c -m32
gcc -shared -o liblighthouse-x64.so -fPIC -ldl lighthouse.c -m64

