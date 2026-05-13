import { Link, Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

const ReaderLayout = () => {
    const { profile, logout, user } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    return (
        <div className="librarian-container">
            <aside className="sidebar">
                <div className="sidebar-logo">
                    <h3>LibPro <span>Reader</span></h3>
                </div>
                <nav className="sidebar-nav">
                    <ul className="librarian-sidebar-menu">
                        <li>
                            <Link to="/reader/books">
                                <span className="menu-icon">🔍</span>
                                <span className="menu-text">Books</span>
                            </Link>
                        </li>
                        <li>
                            <Link to="/reader/my-loans">
                                <span className="menu-icon">📖</span>
                                <span className="menu-text">Borrowed Book History</span>
                            </Link>
                        </li>
                        <li>
                            <Link to="/reader/reservations">
                                <span className="menu-icon">🔖</span>
                                <span className="menu-text">My Reservations</span>
                            </Link>
                        </li>
                        <li>
                            <Link to="/reader/profile">
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

export default ReaderLayout;