#!/usr/bin/env groovy

def runTestsInDocker(container, testFolder, testResultFolderName) {
	container.inside('') {
		try {
	        sh "dotnet test ${testFolder}/PayPal.Core.SDK.NETCore.Tests.csproj --configuration Release --logger \"trx;LogFileName=unittestresults.trx\" -r TestResults"
	    }
		catch (Exception ex) {
			throw ex
		}
		finally {
			// due to MSTestPublisher work incorrectly with docker agent folder we have to copy it into current dir
			sh "cp ${testResultFolderName}/unittestresults.trx ./${testResultFolderName}/netcore.unittestresults.trx || echo \"Absent file ${testResultFolderName}/unittestresults.trx\""
			step([$class: 'MSTestPublisher', testResultsFile:"**/*.trx", failOnError: true, keepLongStdio: true])
		}
	}
}

def getVersionSuffix(branchName) {
	def buildNumber = env.BUILD_NUMBER
	def lastChunk = branchName.substring(branchName.lastIndexOf("/") + 1)
	def suffix = "${lastChunk}-${buildNumber}"
	return suffix
}

node {
	def revision
	def versionSuffix
	def nugetUsername
	def nugetPassword
	
	stage('GetCredentials') {
		withCredentials([usernamePassword(credentialsId: 'nc-artifactory', passwordVariable: 'password', usernameVariable: 'username')]) {
			nugetUsername = username
			nugetPassword = password
		}
	}

	stage('Checkout') {
		deleteDir()

		def scmVars = checkout scm
		revision = scmVars.GIT_COMMIT
		versionSuffix = getVersionSuffix(scmVars.GIT_BRANCH)
	}

	def buildDockerContainer
	def testResultFolderName='Source/UnitTests/TestResults'
	
	stage ('Build') {
		buildDockerContainer = docker.build("build:${revision}", "--no-cache --build-arg version_suffix=${versionSuffix} -f Source/NetCoreBuild.Dockerfile .")
		sh "mkdir ${testResultFolderName}"
	}

	stage ('Run Unit Tests') {
		runTestsInDocker(buildDockerContainer, 'Source/UnitTests', testResultFolderName)
	}

	stage ('Create NuGet package') {
		buildDockerContainer.inside(''){
			sh "dotnet pack Source/SDK/PayPal.Core.SDK.NETCore.csproj -c Release -o Package --version-suffix ${versionSuffix}"
		}
	}

	stage ('Push NuGet package') {
		buildDockerContainer.inside(''){
			sh "dotnet nuget push Source/SDK/Package -s ${env.NUGET_REGISTRY_LINK} -k ${nugetUsername}:${nugetPassword}"
		}
	}
}