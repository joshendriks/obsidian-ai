#!/bin/bash

# Check if the correct number of arguments is passed
if [ "$#" -ne 2 ]; then
  echo "Usage: $0 <filename> <action>"
  exit 1
fi

# Assign arguments to variables
filename=$1
action=$2

# Remove the /app/data prefix from the filename if it exists
trimmed_filename2=${filename#/app/data/}

# Remove the folder I use locally while testing
trimmed_filename=${trimmed_filename2#/Users/joshendriks/src/nwwz/obsidian-ai/data/}

# Use the API_URL environment variable, or default to "http://localhost:5274/"
base_url="${API_URL:-http://localhost:5274}"

# Construct the URL
url="$base_url/$trimmed_filename"

# Perform actions based on the `action` parameter
if [ "$action" == "modified" ]; then
  if [ -f "$filename" ]; then
    payload=$(cat "$filename")
    curl --request POST --url "$url" --header 'Content-Type: multipart/form-data' --form File=@$filename
    echo "File '$filename' has been sent as POST request to '$url'." >> `dirname -- "$( readlink -f -- "$0"; )";`/test.log
  else
    echo "Error: File '$filename' does not exist." >> `dirname -- "$( readlink -f -- "$0"; )";`/test.log
    exit 1
  fi
elif [ "$action" == "deleted" ]; then
  curl -X DELETE "$url"
  echo "DELETE request sent for file '$filename' to '$url'." >> `dirname -- "$( readlink -f -- "$0"; )";`/test.log
else
  echo "Invalid action. Use 'modified' or 'deleted'." >> `dirname -- "$( readlink -f -- "$0"; )";`/test.log
  exit 1
fi
