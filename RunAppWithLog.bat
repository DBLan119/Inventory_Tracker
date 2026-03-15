@echo off
cd /d "%~dp0QuanLyKho\bin\Debug\net8.0-windows"
echo Starting application...
QuanLyKho.exe > error.log 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo Application crashed. Check error.log for details.
    type error.log
    pause
)
