FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
RUN apt-get update -y && apt-get upgrade -y
EXPOSE 80
COPY /out .
RUN ls
ENTRYPOINT ["dotnet","Punc.Api"]