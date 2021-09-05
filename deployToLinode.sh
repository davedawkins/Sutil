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

if [ "$DLLNAME" != "" ]; then
    log "Writing supervisor configuration file into deploy directory"

    cat > ${SOURCEDIR}/${APPNAME}.conf <<EOF
    [program:${APPNAME}]
    command=/usr/bin/dotnet ${TARGETDIR}/${DLLNAME}.dll
    directory=${TARGETDIR}
    autostart=true
    autorestart=true
    stderr_logfile=/var/log/${APPNAME}.err.log
    stdout_logfile=/var/log/${APPNAME}.out.log
    environment=ASPNETCORE_ENVIRONMENT=Production
    user=deploy
    stopsignal=INT
EOF

    log "Adding remote installation script into deploy directory"

    cat > ${SOURCEDIR}/localInstall.sh <<EOF2
    #!/bin/bash
    cp ${TARGETDIR}/${APPNAME}.conf /etc/supervisor/conf.d
    sudo supervisorctl reread
    sudo supervisorctl update
EOF2
fi

log "Sending deploy directory to remote instance"

cout rsync -avz -e 'ssh' ${SOURCEDIR}/. ${USERHOST}:${TARGETDIR} --delete

if [ "$DLLNAME" != "" ]; then
    log "Installing application under supervisor and starting"
    cout ssh ${USERHOST} /bin/bash ${TARGETDIR}/localInstall.sh
fi

log ""
log "Deployment to ${USERHOST} completed"
log $HR
log ""
