import React from 'react';
import { useAuth } from '../context/AuthContext';

const UserProfile: React.FC = () => {
    const { profile } = useAuth();

    if (!profile) {
        return <div style={{ padding: '20px', color: 'white' }}>Loading profile information...</div>;
    }

    return (
        <div className="profile-page-container" style={{ padding: '30px', color: '#fff' }}>
            <h2 style={{ borderBottom: '2px solid #ff4e50', paddingBottom: '10px', marginBottom: '20px' }}>
                Your Profile
            </h2>

            <div className="profile-card" style={{
                background: 'rgba(255,255,255,0.05)',
                padding: '30px',
                borderRadius: '12px',
                display: 'flex',
                alignItems: 'flex-start',
                gap: '20px'
            }}>
                {/* Avatar bên trái */}
                <div style={{
                    width: '100px',
                    height: '100px',
                    borderRadius: '50%',
                    background: '#ff4e50',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    fontSize: '40px',
                    fontWeight: 'bold',
                    flexShrink: 0
                }}>
                    {profile.fullName?.charAt(0).toUpperCase() || profile.username?.charAt(0).toUpperCase()}
                </div>

                {/* Khối văn bản đã được đẩy sang trái và căn lề trái */}
                <div style={{
                    display: 'flex',
                    flexDirection: 'column',
                    gap: '15px',
                    textAlign: 'left' 
                }}>
                    <div>
                        <label style={{ color: '#888', fontSize: '11px', display: 'block', marginBottom: '4px', textTransform: 'uppercase' }}>
                            Full Name
                        </label>
                        <div style={{ fontSize: '18px', fontWeight: 'bold' }}>
                            {profile.fullName || "Not updated"}
                        </div>
                    </div>

                    <div>
                        <label style={{ color: '#888', fontSize: '11px', display: 'block', marginBottom: '4px', textTransform: 'uppercase' }}>
                            Username
                        </label>
                        <div style={{ fontSize: '16px' }}>@{profile.username}</div>
                    </div>

                    <div>
                        <label style={{ color: '#888', fontSize: '11px', display: 'block', marginBottom: '4px', textTransform: 'uppercase' }}>
                            Email
                        </label>
                        <div style={{ fontSize: '16px' }}>{profile.email}</div>
                    </div>

                    <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                        <label style={{ color: '#888', fontSize: '11px', textTransform: 'uppercase' }}>
                            Role:
                        </label>
                        <span style={{
                            padding: '4px 12px',
                            background: '#ff4e50',
                            borderRadius: '6px',
                            fontSize: '12px',
                            fontWeight: 'bold'
                        }}>
                            {profile.role}
                        </span>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default UserProfile;