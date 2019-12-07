#!/usr/bin/env bash
set -ev

TAG=$1
NUGET_API_KEY=$2
VERSION=`echo ${TAG} | cut -b 2-`

src_path=./AElf.Client
build_output=/tmp/aelf-build

if [[ -d ${build_output} ]]; then
    rm -rf ${build_output}
fi

dotnet restore AElf.Client.sln

echo '---- build '${src_path}

name=AElf.Client
do
    echo ${name}/${name}.csproj
    dotnet build ${name}/${name}.csproj /clp:ErrorsOnly --configuration Release -P:Version=${VERSION} -P:Authors=AElf -o ${build_output}

    echo `ls ${build_output}/${name}.${VERSION}.nupkg`
    dotnet nuget push ${build_output}/${name}.${VERSION}.nupkg -k ${NUGET_API_KEY} -s https://api.nuget.org/v3/index.json
done
cd ../
