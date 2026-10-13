@echo off
setlocal
py -3 -m pip install --upgrade pip
py -3 -m pip install -r requirements.txt pyinstaller
py -3 -m PyInstaller --noconfirm --clean --onefile --windowed --name CreviceHeroPC --add-data "assets;assets" --add-data "reference/original_data;reference/original_data" crevice_hero.py
if errorlevel 1 exit /b 1
echo Build complete: dist\CreviceHeroPC.exe
