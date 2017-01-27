#!/bin/sh

set -e

NUSPEC_FILE=OSMDataPrimitives.BSON.nuspec
LINUX_NUSPEC_FILE=OSMDataPrimitives.BSON.linux.nuspec
SED_REGEX='s/\(="\)\([^"]*\)\\\([^"]*\)/\1\2\/\3/g'
cat ${NUSPEC_FILE} | sed -e ${SED_REGEX} | sed -e ${SED_REGEX} | sed -e ${SED_REGEX} > ${LINUX_NUSPEC_FILE}
NuGet.exe pack ${LINUX_NUSPEC_FILE} -properties Configuration=Release

rm ${LINUX_NUSPEC_FILE}

exit
