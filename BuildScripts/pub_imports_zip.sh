#!/bin/bash

CALL=$0

usage() {
  echo "Use one of these: " 2>&1
  echo "  $CALL server_name" 2>&1
  echo "  SERVER=server_name" 2>&1
}

if [[ "$1" == "" ]] && [[ "$SERVER" == "" ]]; then
  usage

  exit 1
fi

scp -r bin/imports.zip $SERVER:/var/www/dregus/mage-awakened/

