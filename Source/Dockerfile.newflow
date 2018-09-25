FROM nc-mss-app-docker.artifactory.corp.namecheap.net/nc/ncpl/ncpl-build:dotnet2.1

WORKDIR /

COPY . .

RUN dotnet build Source/SDK/PayPal.Core.SDK.NETCore.csproj -c Release --version-suffix "$version_suffix"