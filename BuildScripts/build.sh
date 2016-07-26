#!/bin/bash

CALL=$0

usage() {
	echo "Use one of these (replace vX.X.X with v0.0.1 or whichever version): " 2>&1
	echo "  $CALL vX.X.X" 2>&1
	echo "  VERSION=vX.X.X" 2>&1
}

if [[ "$1" == "" ]] && [[ "$VERSION" == "" ]]; then
	usage

	exit 1
fi

if [ -d "bin" ]; then
	mkdir -p "$VERSION"

	zip -9 "$VERSION/dregus/mage-awakened.zip" bin/*exe bin/*Data
	rm -rf bin/*exe bin/*Data

	mkdir -p bin/builds
	mv "$VERSION" bin/builds/
else
	echo "Expected a bin directory at the pwd: $(pwd)"
fi

