#!/bin/bash
gcc -shared -o liblighthouse-x86.dylib -fPIC -ldl lighthouse.c -m32
gcc -shared -o liblighthouse-x64.dylib -fPIC -ldl lighthouse.c -m64

