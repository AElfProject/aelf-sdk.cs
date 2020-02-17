#!/bin/bash
dir=`pwd`
ip=`ip a | grep eth0 |grep 'inet' | awk -F/ '{print $1}'| awk '{print $2}'`
sudo sed -i "s/172.31.8.57/$ip/g" appsettings.json 
sudo apt update &&apt-get install make autoconf  build-essential   gcc unzip -y >>/dev/null
cd /opt && sudo wget --no-check-certificate https://github.com/ideawu/ssdb/archive/master.zip >>/dev/null
cd /opt &&sudo  unzip master.zip && cd /opt/ssdb-master/ &&sudo  make >>/dev/null &&  sudo make install PREFIX=/opt/ssdb-template /dev/null && cd ..
sudo sed -i 's#127.0.0.1#0.0.0.0#g' /opt/ssdb-template/ssdb.conf
sudo sed -i 's#debug#error#g'       /opt/ssdb-template/ssdb.conf
sudo sed -i 's#8888#6379#g'       /opt/ssdb-template/ssdb.conf
sudo /opt/ssdb-template/ssdb-server -d  /opt/ssdb-template/ssdb.conf
sudo docker pull aelf/node:testnet-v0.9.2
sudo docker run -itd --name aelf-node-test -v $dir:/opt/node -v $dir/keys:/root/.local/share/aelf/keys -p 8001:8000 -p 6800:6800 -w /opt/node aelf/node:testnet-v0.9.2 dotnet /app/AElf.Launcher.dll
sleep 30
height=`curl -s http://$ip:8001/api/blockChain/blockHeight`
echo "height is $height"
cd $dir
