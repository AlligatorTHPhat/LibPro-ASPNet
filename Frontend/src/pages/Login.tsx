export default function LoginPage() {
    return (
        <div style={{ padding: '20px', textAlign: 'center' }}>
            <h2>Đăng Nhập Hệ Thống</h2>
            <input type="text" placeholder="Username" /><br /><br />
            <input type="password" placeholder="Password" /><br /><br />
            <button>Login</button>
        </div>
    );
}