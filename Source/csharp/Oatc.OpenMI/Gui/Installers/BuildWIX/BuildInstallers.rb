def make_installer
	candle='C:\Program Files\Windows Installer XML v3\bin\candle'
	light='C:\Program Files\Windows Installer XML v3\bin\light'
	wixUI='"C:\Program Files\Windows Installer XML v3\bin\wixUI.wixlib"'
	
	build = "Debug x86"
  	product = "OATC_OpenMI_Editor_x86_2_0_AlphaX"
	xmlFiles = ["Editor"]

# WARNING
# Need to copy help files 
# from ..\Help
# to ..\ConfigurationEditor

	wixs = []
	wixobjs = []

	xmlFiles.each { |f| wixs.push f + ".wxs" }
	xmlFiles.each { |f| wixobjs.push f + ".wixobj" }

#	return if !system_command("Building #{build}", "Make.bat #{build}")  
	return if !system_command("Making Candle", "#{candle} #{wixs.join(" ")}")  
	return if !system_command("Making Light",  "#{light}  #{wixobjs.join(" ")} -cultures:en-us -ext WixUIExtension -out \"#{product}.msi\"")  

	STDOUT.puts "Installer Make completed OK"
	STDOUT.flush
end

def system_command(comment, cmd)
	STDOUT.puts "\n#{comment} ...\n"    
	STDOUT.flush
	STDOUT.puts `#{cmd}`
	return true if $? == 0
	STDERR.puts "\n===FAILED=== #{comment}\n"
	return false
end

make_installer()



