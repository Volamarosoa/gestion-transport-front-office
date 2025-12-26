pipeline {
    agent any  // Agent principal pour Jenkins

    environment {
        APP_NAME   = "gestiontransport-frontoffice"
        IMAGE_NAME = "gestiontransport-frontoffice:dotnet9"
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore') {
            agent {
                docker {
                    image 'mcr.microsoft.com/dotnet/sdk:9.0'
                    args '-u 0:0'
                    reuseNode true
                }
            }
            steps {
                sh 'dotnet restore gestion-transport.sln'
            }
        }

        stage('Build') {
            agent {
                docker {
                    image 'mcr.microsoft.com/dotnet/sdk:9.0'
                    args '-u 0:0'
                    reuseNode true
                }
            }
            steps {
                sh 'dotnet build gestion-transport.sln -c Release --no-restore'
            }
        }

        stage('Tests') {
            agent {
                docker {
                    image 'mcr.microsoft.com/dotnet/sdk:9.0'
                    args '-u 0:0'
                    reuseNode true
                }
            }
            steps {
                sh 'dotnet test gestion-transport.sln -c Release --no-build'
            }
        }

        stage('Publish') {
            agent {
                docker {
                    image 'mcr.microsoft.com/dotnet/sdk:9.0'
                    args '-u 0:0'
                    reuseNode true
                }
            }
            steps {
                sh '''
                dotnet publish GestionTransport.FrontOffice/GestionTransport.FrontOffice.csproj \
                  -c Release -o publish
                '''
            }
        }

        stage('Docker Build') {
            steps {
                sh 'docker build -t $IMAGE_NAME .'
            }
        }

        stage('Deploy (simulation)') {
            steps {
                sh '''
                docker rm -f $APP_NAME || true
                docker run -d -p 8081:8080 --name $APP_NAME $IMAGE_NAME
                '''
            }
        }
    }
}