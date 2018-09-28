FROM microsoft/dotnet:2.1-sdk

WORKDIR /

COPY . .

RUN dotnet build Source/SDK/PayPal.Core.SDK.NETCore.csproj -c Release --version-suffix "$version_suffix"