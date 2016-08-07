#!/bin/bash

CALL=$0

usage() {
  echo "Use one of these (replace vX.X.X with v0.0.1 or whichever version): " 2>&1
  echo "  $CALL vX.X.X" 2>&1
  echo "  VERSION=vX.X.X" 2>&1
}

if [[ "$1" != "" ]]; then
  VERSION=$1
fi

if [[ "$VERSION" == "" ]]; then
  usage

  exit 1
fi

if [ -d "bin" ]; then
  WORKING_DIR="bin"

  cd $WORKING_DIR
  OUTPUT_DIR="builds/dregus/$VERSION"
  OUTPUT_FILE="$OUTPUT_DIR/mage-awakened.zip"

  mkdir -p "$OUTPUT_DIR"

  mkdir Dregus
  mv *exe Dregus/
  mv *Data Dregus/

  rm -f "$OUTPUT_FILE"
  if ! zip -r -9 "$OUTPUT_FILE" Dregus/*exe Dregus/*Data; then
    exit $?
  fi

  if [ "$(git branch | grep '^*')" == "* master" ]; then
    git tag $VERSION
    git push --tags
  fi

  rm -rf Dregus

  ls -hal "$OUTPUT_FILE"
  cd -
else
  echo "Expected a bin directory at the pwd: $(pwd)"
fi

