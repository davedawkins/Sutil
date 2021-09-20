#!/bin/bash

#
# Requirements on remote host (eg linode running Ubuntu 20.04)
# - apt-get install supervisor
# - USERHOST has password-less login enabled. Use ssh-keygen -t rsa / ssh-copy-key
# - USERHOST has user deploy enabled with in /etc/sudoers (use visudo for this)
#       %deploy ALL=(ALL:ALL) NOPASSWD: /usr/bin/supervisorctl
# - /etc/supervisor/conf.d has group "deploy", mode 0775 (so we can copy conf files)
#
# See https://docs.servicestack.net/netcore-deploy-rsync

SOURCEDIR=$1
APPNAME=$2
DLLNAME=$3
USERHOST=$4
APPDIR=$5

TARGETDIR=${APPDIR}/${APPNAME}

# https://stackoverflow.com/questions/5947742/how-to-change-the-output-color-of-echo-in-linux

HR="---------------------------------------------------------------------"
GREEN='\033[0;32m'
LGREEN='\033[1;32m'
BLUE='\033[0;34m'
LBLUE='\033[1;34m'
DGRAY='\033[1;30m'
NC='\033[0m'

cecho()
{
    echo -e ${LBLUE}$*${NC}
}

cout()
{
    $* 2>&1 |
    while read line; do
        echo -e "${DGRAY}${line}"
    done
}

log()
{
    cecho $*
}

log ""
log $HR
log "Deploying ${APPNAME} to ${USERHOST}:/${TARGETDIR}"
log $HR
log ""
log "Adding remote installation script into deploy directory"

cat > ${SOURCEDIR}/localInstall.sh <<EOF2
#!/bin/bash
ln -s ${TARGETDIR}/../repl ${TARGETDIR}/repl
EOF2

log "Sending deploy directory to remote instance"

cout rsync -avz -e 'ssh' ${SOURCEDIR}/. ${USERHOST}:${TARGETDIR} --delete

log "Executing localInstall.sh"
cout ssh ${USERHOST} /bin/bash ${TARGETDIR}/localInstall.sh

log ""
log "Deployment to ${USERHOST} completed"
log $HR
log ""
