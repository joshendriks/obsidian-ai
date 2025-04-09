def deploy = {
	sh 'mkdir -p /home/sr/notes'
	sh 'mkdir -p /home/sr/data/notes/'
	sh 'mkdir -p /home/sr/data/notes/live-syc/data'
	sh 'mkdir -p /home/sr/data/notes/live-sync/dat'
	sh 'mkdir -p /home/sr/data/notes/live-sync/script'
	sh 'mkdir -p /home/sr/data/notes/redis'
    sh 'mkdir -p /home/sr/notes/redis/config'
    sh 'cp redis/redis-server.conf /home/sr/notes/redis/config/redis-server.conf'
	sh 'cp script/*.* /home/sr/data/notes/live-sync/script/'
	sh 'cp docker-compose-server.yml /home/sr/notes/docker-compose.yml'
	dir("/home/sr/notes") {
		sh 'docker compose pull'
		sh 'docker compose down'
		sh 'docker compose up -d'
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
