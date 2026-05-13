import { Link, Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

const LibrarianLayout = () => {
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
                    <h3>LibPro <span>Admin</span></h3>
                </div>
                <nav className="sidebar-nav">
                    <ul>
                        <ul className="librarian-sidebar-menu">
                            <li>
                                <Link to="/librarian/categories">
                                    <span className="menu-icon">📁</span>
                                    <span className="menu-text">Category Management</span>
                                </Link>
                            </li>
                            <li>
                                <Link to="/librarian/books">
                                    <span className="menu-icon">📚</span>
                                    <span className="menu-text">Book Management</span>
                                </Link>
                            </li>
                            <li>
                                <Link to="/librarian/readers">
                                    <span className="menu-icon">👥</span>
                                    <span className="menu-text">Reader Management</span>
                                </Link>
                            </li>
                            <li>
                                <Link to="/librarian/reservations">
                                    <span className="menu-icon">📅</span>
                                    <span className="menu-text">Reservation Management</span>
                                </Link>
                            </li>
                            <li>
                                <Link to="/librarian/loans">
                                    <span className="menu-icon">📝</span>
                                    <span className="menu-text">Loan Management</span>
                                </Link>
                            </li>
                            <li>
                                <Link to="/librarian/publishers">
                                    <span className="menu-icon">🖨️</span>
                                    <span className="menu-text">Publisher Management</span>
                                </Link>
                            </li>
                            <li>
                                <Link to="/librarian/suppliers">
                                    <span className="menu-icon">🏢</span>
                                    <span className="menu-text">Supplier Management</span>
                                </Link>
                            </li>
                            <li>
                                <Link to="/librarian/profile">
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
                    </ul>
                </nav>
            </aside>
            <main className="main-content">
                <Outlet />
            </main>
        </div>
    );
};

export default LibrarianLayout;