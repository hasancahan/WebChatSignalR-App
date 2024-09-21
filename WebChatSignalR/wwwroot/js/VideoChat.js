const roomNameInput = document.getElementById('roomHashInput');
const shareRoomHash = document.getElementById('shareRoomHash');
const localVideo = document.getElementById('localVideo');
const remoteVideo = document.getElementById('remoteVideo');
let drone;
let room;
let pc;
let isOfferer = false;
let iceCandidatesQueue = []; // ICE candidate'lar� saklamak i�in bir dizi

function startWebRTC() {
    const roomName = roomNameInput.value.trim();
    if (!roomName) {
        alert('Oda ad� bo� olamaz!');
        return;
    }

    drone = new ScaleDrone('dhFM5YZvDfCJpHWn');
    const configuration = {
        iceServers: [{ urls: 'stun:stun.l.google.com:19302' }]
    };

    drone.on('open', error => {
        if (error) {
            console.error('ScaleDrone ba�lant� hatas�:', error);
            return;
        }
        room = drone.subscribe('observable-' + roomName);
        room.on('open', error => {
            if (error) {
                console.error('Odaya giri� hatas�:', error);
            }
        });

        room.on('members', members => {
            console.log('MEMBERS', members);
            isOfferer = members.length === 2;
            initWebRTC(configuration);
        });

        room.on('data', (message, client) => {
            if (client.id === drone.clientId) return;

            if (message.sdp) {
                pc.setRemoteDescription(new RTCSessionDescription(message.sdp), () => {
                    if (pc.remoteDescription.type === 'offer') {
                        pc.createAnswer().then(localDescCreated).catch(onError);
                    }
                    // Uzak tan�m ayarland���nda bekleyen ICE candidate'lar� i�le
                    iceCandidatesQueue.forEach(candidate => {
                        pc.addIceCandidate(candidate).catch(onError);
                    });
                    iceCandidatesQueue = []; // ��lenen candidate'lar� temizle
                }, onError);
            } else if (message.candidate) {
                // Uzak tan�m ayarlanmad�ysa candidate'lar� beklet
                if (pc.remoteDescription) {
                    pc.addIceCandidate(new RTCIceCandidate(message.candidate)).catch(onError);
                } else {
                    console.warn("Remote description hen�z ayarlanmad�, ICE candidate bekletiliyor.");
                    iceCandidatesQueue.push(new RTCIceCandidate(message.candidate)); // ICE candidate'� kuyru�a ekle
                }
            }
        });
    });
}

function initWebRTC(configuration) {
    pc = new RTCPeerConnection(configuration);

    pc.onicecandidate = event => {
        if (event.candidate) {
            sendMessage({ candidate: event.candidate });
        }
    };

    pc.ontrack = event => {
        const stream = event.streams[0];
        if (!remoteVideo.srcObject || remoteVideo.srcObject.id !== stream.id) {
            remoteVideo.srcObject = stream;
        }
    };

    navigator.mediaDevices.getUserMedia({
        audio: true,
        video: true
    }).then(stream => {
        localVideo.srcObject = stream;
        stream.getTracks().forEach(track => pc.addTrack(track, stream));
    }).catch(onError);

    if (isOfferer) {
        pc.onnegotiationneeded = () => {
            pc.createOffer().then(localDescCreated).catch(onError);
        };
    }
}

function localDescCreated(desc) {
    pc.setLocalDescription(desc, () => {
        sendMessage({ sdp: pc.localDescription });
    }, onError);
}

function sendMessage(message) {
    drone.publish({
        room: 'observable-' + roomNameInput.value.trim(),
        message
    });
}

function onError(error) {
    console.error('Hata:', error);
}

function startCall() {
    const roomName = roomNameInput.value.trim();
    if (!roomName) {
        alert('Oda ad� bo� olamaz!');
        return;
    }
    shareRoomHash.value = roomName;
    startWebRTC();
}
