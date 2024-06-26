FROM mcr.microsoft.com/vscode/devcontainers/base:0-focal

COPY custom-scripts/setup.sh /conf/setup.sh

# Enable immediately list of possible values if terminal input is ambiguous
RUN echo 'set show-all-if-ambiguous on' >> /etc/inputrc
# Run through all possible values by using Shift+Tab
RUN echo '"\\e[Z": menu-complete' >> /etc/inputrc

ENV \
	# Enable detection of running in a container
	DOTNET_RUNNING_IN_CONTAINER=true \
	# Enable correct mode for dotnet watch (only mode supported in a container)
	DOTNET_USE_POLLING_FILE_WATCHER=true \
	# Skip extraction of XML docs - generally not useful within an image/container - helps performance
	NUGET_XMLDOC_MODE=skip

# Install .Net required libraries
RUN apt-get update \
	&& apt-get install -y wget

# Set up .Net repository
RUN wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
	&& dpkg -i packages-microsoft-prod.deb

# Install transport-https
RUN apt-get update \
	&& apt-get install -y apt-transport-https

# Install .Net Core and Entity Framework tools
RUN apt-get update \
	&& apt-get install -y dotnet-sdk-6.0 dotnet-sdk-8.0 \
	&& dotnet help
