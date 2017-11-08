#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=JetBrains.dotCover.CommandLineTools"
#addin "nuget:?package=Rocket.Surgery.Build.Cake&version=0.1.5"
#load "nuget:?package=Rocket.Surgery.Build.Cake&version=0.1.5";

Task("Default")
    .IsDependentOn("GitVersion")
    .IsDependentOn("CleanArtifacts")
    .IsDependentOn("DotnetCore");

RunTarget(target);
