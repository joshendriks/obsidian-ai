def deploy = {
	sh 'mkdir -p /home/sr/notes'
	sh 'mkdir -p /home/sr/data/notes/'
	sh 'mkdir -p /home/sr/data/notes/live-sync/data'
	sh 'mkdir -p /home/sr/data/notes/live-sync/dat'
	sh 'mkdir -p /home/sr/data/notes/live-sync/script'
	sh 'mkdir -p /home/sr/data/notes/redis'
    sh 'mkdir -p /home/sr/notes/redis/config'
    sh 'cp redis/redis-server.conf /home/sr/notes/redis/config/redis-server.conf'
	sh 'cp script/*.* /home/sr/data/notes/live-sync/script/'
	sh 'cp docker-compose-server.yml /home/sr/notes/docker-compose.yml'
    withCredentials([string(credentialsId: 'cf94de35-0bd9-4cda-898c-b2ea4c72a4c0', variable: 'dockerKey')]) {
        sh 'echo ${dockerKey} | docker login ghcr.io -u joshendriks --password-stdin'
        dir("/home/sr/notes") {
            sh 'docker compose pull'
            sh 'docker compose up -d'
        }
        sh 'docker logout'
    }
}

pipeline {
	agent none

    stages {
		stage('Deploy') {
			agent { label 'soyo' }
            steps {
                script {deploy ()}
            }
        }
    }
}
