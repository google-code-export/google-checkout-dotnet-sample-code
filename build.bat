@echo off
msbuild /verbosity:minimal %* build.proj /p:Configuration=Release