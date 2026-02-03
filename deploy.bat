set destFolderPath=%1
set remoteMachine=%2
set uName=%3
set pwd=%4
set buildConfig=%5
set project=%6
set sourceFolderPath=%cd%\src\%project%\bin\%buildConfig%

echo %destFolderPath%
echo %remoteMachine%
echo %uName%
echo %sourceFolderPath%
echo %buildConfig%

echo 'starting build'

%systemroot%\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe src\%project%\%project%.csproj /p:Configuration=%buildConfig% /p:OutputPath=bin/%buildConfig%

