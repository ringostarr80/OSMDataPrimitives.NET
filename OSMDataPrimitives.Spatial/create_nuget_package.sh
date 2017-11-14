#!/bin/sh

set -e

NUSPEC_FILE=OSMDataPrimitives.Spatial.nuspec
LINUX_NUSPEC_FILE=OSMDataPrimitives.Spatial.linux.nuspec
SED_REGEX='s/\(="\)\([^"]*\)\\\([^"]*\)/\1\2\/\3/g'
cat ${NUSPEC_FILE} | sed -e ${SED_REGEX} | sed -e ${SED_REGEX} | sed -e ${SED_REGEX} > ${LINUX_NUSPEC_FILE}

#msbuild /property:Configuration=Release
#NuGet.exe pack ${LINUX_NUSPEC_FILE} -properties Configuration=Release
dotnet pack --configuration=Release

rm ${LINUX_NUSPEC_FILE}

exit
