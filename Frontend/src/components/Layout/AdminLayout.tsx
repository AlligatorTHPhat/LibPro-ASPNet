import { Link, Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

const AdminLayout = () => {
    const { profile, logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/login');
    };

    return (
        <div className="librarian-container">
            <aside className="sidebar">
                <div className="sidebar-logo">
                    <h3>LibPro <span style={{ color: '#ff4e50' }}>Director</span></h3>
                </div>

                <nav className="sidebar-nav">
                    <ul className="librarian-sidebar-menu">
                        <li>
                            <Link to="/admin/dashboard">
                                <span className="menu-icon">📈</span>
                                <span className="menu-text">Report</span>
                            </Link>
                        </li>
                        <li>
                            <Link to="/admin/librarians">
                                <span className="menu-icon">👔</span>
                                <span className="menu-text">Librarian Management</span>
                            </Link>
                        </li>
                        <li>
                            <Link to="/admin/system-logs">
                                <span className="menu-icon">⚙️</span>
                                <span className="menu-text">Audit Log</span>
                            </Link>
                        </li>
                        <li>
                            <Link to="/admin/profile">
                                <span className="menu-icon">👤</span>
                                <span className="menu-text">My Profile</span>
                            </Link>
                        </li>
                        <li className="logout-item">
                            <a href="#" onClick={handleLogout}>
                                <span className="menu-icon">🚪</span>
                                <span className="menu-text">Logout</span>
                            </a>
                        </li>
                    </ul>
                </nav>
            </aside>
            <main className="main-content">
                <Outlet />
            </main>
        </div>
    );
};

export default AdminLayout;