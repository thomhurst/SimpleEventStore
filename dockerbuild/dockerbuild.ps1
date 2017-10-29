Param(
    [string]$uri,
    [string]$authKey,
    [string]$consistencyLevel,
    [string]$buildVersion
)

docker build -t ses-build -f DockerFile.build ../ `
--build-arg uri=$uri `
--build-arg authKey=$authKey `
--build-arg consistencyLevel=$consistencyLevel `
--build-arg buildVersion=$buildVersion