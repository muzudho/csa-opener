#############
# Locations #
#############

$original = "C:\Users\�ނ��ł�\source\repos\shogi-kifu-converter"
$deploy   = "C:\Users\�ނ��ł�\Documents\GitHub\shogi-kifu-converter";

#######
# Lib #
#######

# �t�@�C���̍폜�B
function Remove-File($dst) {
	if (Test-Path $dst) {
		Write-Host "Delete  : [$($dst)]."
		Remove-Item $dst
	}
}

# �f�B���N�g���[�̍폜�B
function Remove-Dir($dst) {
	if (Test-Path $dst) {
		Write-Host "Delete  : [$($dst)] directory."
		Remove-Item $dst -Recurse
	}
}

# �t�@�C���̃R�s�[�B
function Copy-File($src, $dst) {
	Write-Host "Copy    : [$($src)] --> [$($dst)]."
	Copy-Item $src $dst
}

# �f�B���N�g���[�̃R�s�[�B
function Copy-Dir($src, $dst) {
	Write-Host "Copy    : [$($src)] --> [$($dst)] directory."
	Copy-Item $src $dst -Recurse
}

# �t�@�C���̈ړ��B
function Move-File($src, $dst) {
	if (Test-Path $src) {
		Write-Host "Move    : [$($src)] --> [$($dst)]."
		Move-Item $src $dst
	}
}

# �f�B���N�g���[�̈ړ��B
function Move-File($src, $dst) {
	if (Test-Path $src) {
		Write-Host "Move    : [$($src)] --> [$($dst)]."
		Move-Item $src $dst -Recurse
	}
}

######
# Go #
######

##
 # Note: Not trailer slash.
 ##

Remove-Dir  "$($deploy)\-- GENERATOR"
Remove-Dir  "$($deploy)\visual-studio"

Remove-File "$($deploy)\.gitignore"
Remove-File "$($deploy)\LICENSE"
Remove-File "$($deploy)\README.md"

Copy-Dir    "$($original)\-- GENERATOR"                      "$($deploy)\-- GENERATOR"
Copy-Dir    "$($original)\visual-studio"                     "$($deploy)\visual-studio"

Copy-File   "$($original)\.gitignore"                        "$($deploy)\.gitignore"
Copy-File   "$($original)\LICENSE"                           "$($deploy)\LICENSE"
Copy-File   "$($original)\README.md"                         "$($deploy)\README.md"

pause