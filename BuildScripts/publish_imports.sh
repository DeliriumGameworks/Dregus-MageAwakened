#!/bin/bash

CALL=$0

usage() {
  echo "Use one of these: " 2>&1
  echo "  $CALL server_name" 2>&1
  echo "  SERVER=server_name" 2>&1
}

if [[ "$1" != "" ]]; then
  SERVER=$1
fi

if [[ "$SERVER" == "" ]]; then
  usage

  exit 1
fi

rsync -v -c -a --append-verify -L Assets/imports $USER@$SERVER:/home/$USER

