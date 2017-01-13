#!/bin/bash
set -ev

echo "Building using dummy assemblies from KSP-KOS/KSP_LIB"
wget --quiet https://github.com/KSP-KOS/KSP_LIB/blob/master/kos-${KSP_VERSION}.tar?raw=true -O lib-${KSP_VERSION}.tar

tar -xvf lib-${KSP_VERSION}.tar

buildtag=${TRAVIS_TAG}
if [[ ! -v $TRAVIS_TAG ]]; then 
    buildtag=${TRAVIS_COMMIT}
fi

sed -i "s/\(.*PROGRAM_VERSION = \"\)Custom\(\".*\)/\1${buildtag}\2/g" Common/Common.cs