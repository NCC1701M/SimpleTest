ARG OS_DIST=ubuntu
ARG OS_VERSION=22.04

FROM mcr.microsoft.com/vscode/devcontainers/base:${OS_DIST}-${OS_VERSION}

# To use the arguments inside the build stage the ARG instruction has to be used again without assigning a value.
ARG OS_DIST
ARG OS_VERSION

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
	NUGET_XMLDOC_MODE=skip \
	# Sets the .Net version to be used
	DOTNET_VERSION=8.0

# Install .Net SDK
RUN apt-get update \
	&& apt-get install -y dotnet-sdk-${DOTNET_VERSION}
