pipeline {
    agent any
    
    stages {
        stage('Setup Git Safe Directory') {
            steps {
                withCredentials([
                    string(credentialsId: 'PROJECT_PATH', variable: 'PROJECT_PATH')
                ]) {
                    script {
                        echo 'Configuring Git safe directory...'
                        sh '''
                            sudo git config --global --add safe.directory $PROJECT_PATH
                        '''
                    }
                }
            }
        }
        
        stage('Pull Latest Code') {
            steps {
                withCredentials([
                    string(credentialsId: 'PROJECT_PATH', variable: 'PROJECT_PATH'),
                    string(credentialsId: 'TARGET_BRANCH', variable: 'BRANCH_NAME')
                ]) {
                    script {
                        echo 'Pulling latest changes from repository...'
                        sh '''
                            cd $PROJECT_PATH
                            sudo git pull origin $BRANCH_NAME
                        '''
                    }
                }
            }
        }
        
        stage('Build and Deploy') {
            steps {
                withCredentials([
                    string(credentialsId: 'PROJECT_PATH', variable: 'PROJECT_PATH')
                ]) {
                    script {
                        echo 'Building and deploying with Docker Compose...'
                        sh '''
                            cd $PROJECT_PATH
                            sudo docker-compose down
                            sudo docker-compose up -d --build
                        '''
                    }
                }
            }
        }
        
        stage('Verify Deployment') {
            steps {
                withCredentials([
                    string(credentialsId: 'PROJECT_PATH', variable: 'PROJECT_PATH')
                ]) {
                    script {
                        echo 'Verifying containers are running...'
                        sh '''
                            sudo docker-compose -f $PROJECT_PATH/docker-compose.yml ps
                        '''
                    }
                }
            }
        }
    }
    
    post {
        success {
            echo 'Deployment completed successfully!'
        }
        failure {
            echo 'Deployment failed!'
        }
    }
}