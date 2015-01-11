%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild PDM.sln /t:Clean /p:Configuration=Debug /nologo /clp:NoSummary /m
%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild PDM.sln /t:Rebuild /p:Configuration=Debug /nologo /clp:NoSummary /m
pause