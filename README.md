#FileDownloader
This application has command line interface and once it is executed, it automatically downloads all the files with ".xml" extension
for each company and saves them to a zipped folder.

User provides the the list of http links separated by white space as a command line arguments for each company.
For each http link, ftp link is generated. Ftp link is then used to search for required files and to download them put them in zipped folder
with name following the special, pre-defined format
