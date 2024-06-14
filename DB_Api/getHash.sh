#!/bin/bash

# Extract the last 8 digits of the commit hash
hash=$(echo $GITHUB_SHA | cut -c1-8)

# Output the last 8 digits of the commit hash
echo $hash