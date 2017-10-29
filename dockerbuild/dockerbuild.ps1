Param(
    [string]$uri,
    [string]$authKey,
    [string]$consistencyLevel,
    [string]$buildVersion="1.0.0"
)

docker build -t ses-build -f DockerFile.build ../ `
--build-arg uri=$uri `
--build-arg authKey=$authKey `
--build-arg consistencyLevel=$consistencyLevel `
--build-arg buildVersion=$buildVersion `
--no-cache `
--rm