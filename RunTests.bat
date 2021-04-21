@echo off
"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe" "E:\Projects\ProjectX\ProjectX.Test\ProjectX.Test.sln" /t:Clean;Build /p:Configuration=Release /p:Platform=x86
"E:\Projects\ProjectX\ProjectX.Test\ProjectX.Test\bin\x86\Release\ProjectX.Test.exe" 