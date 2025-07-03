// SignalR Client Management
class SignalRClient {
    constructor() {
        this.connection = null;
        this.isConnected = false;
        this.reconnectAttempts = 0;
        this.maxReconnectAttempts = 5;
        this.init();
    }

    init() {
        // Create SignalR connection
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/newsHub")
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .build();

        // Set up event handlers
        this.setupEventHandlers();
        
        // Start connection
        this.startConnection();
    }

    setupEventHandlers() {
        // Connection events
        this.connection.onreconnecting((error) => {
            console.log("SignalR: Reconnecting...", error);
            this.isConnected = false;
        });

        this.connection.onreconnected((connectionId) => {
            console.log("SignalR: Reconnected with connection ID:", connectionId);
            this.isConnected = true;
            this.reconnectAttempts = 0;
        });

        this.connection.onclose((error) => {
            console.log("SignalR: Connection closed", error);
            this.isConnected = false;
        });

        // News events
        this.connection.on("NewsCreated", (newsData) => {
            console.log("SignalR: News created", newsData);
            this.handleNewsCreated(newsData);
        });

        this.connection.on("NewsUpdated", (newsData) => {
            console.log("SignalR: News updated", newsData);
            this.handleNewsUpdated(newsData);
        });

        this.connection.on("NewsDeleted", (newsId) => {
            console.log("SignalR: News deleted", newsId);
            this.handleNewsDeleted(newsId);
        });
    }

    async startConnection() {
        try {
            await this.connection.start();
            console.log("SignalR: Connected successfully");
            this.isConnected = true;
        } catch (err) {
            console.error("SignalR: Connection failed", err);
            this.isConnected = false;
            
            // Retry connection
            if (this.reconnectAttempts < this.maxReconnectAttempts) {
                this.reconnectAttempts++;
                setTimeout(() => this.startConnection(), 2000);
            }
        }
    }

    handleNewsCreated(newsData) {
        // Show notification
        this.showNotification(`News mới: ${newsData.title}`, 'success');
        
        // Page-specific handling
        const currentPath = window.location.pathname;
        
        if (currentPath.includes('/Admin/Dashboard')) {
            this.updateAdminDashboard(newsData);
        } else if (currentPath.includes('/Staff/Dashboard')) {
            this.updateStaffDashboard(newsData);
        } else if (currentPath.includes('/News/Index')) {
            this.updateNewsIndex(newsData);
        }
    }

    handleNewsUpdated(newsData) {
        this.showNotification(`News đã được cập nhật: ${newsData.title}`, 'info');
        
        // Reload page after 2 seconds to show updated data
        setTimeout(() => {
            window.location.reload();
        }, 2000);
    }

    handleNewsDeleted(newsId) {
        this.showNotification("News đã được xóa", 'warning');
        
        // Reload page after 2 seconds to show updated data
        setTimeout(() => {
            window.location.reload();
        }, 2000);
    }

    updateAdminDashboard(newsData) {
        // Update news count
        const newsCountElement = document.getElementById('newsCount');
        if (newsCountElement) {
            const currentCount = parseInt(newsCountElement.textContent);
            newsCountElement.textContent = currentCount + 1;
        }
        
        // Add new news to the top of the list
        const recentNewsList = document.getElementById('recentNewsList');
        if (recentNewsList) {
            const listGroup = recentNewsList.querySelector('.list-group');
            if (listGroup) {
                const newNewsItem = document.createElement('div');
                newNewsItem.className = 'list-group-item d-flex justify-content-between align-items-start news-item';
                newNewsItem.setAttribute('data-news-id', newsData.id);
                newNewsItem.innerHTML = `
                    <div class="ms-2 me-auto">
                        <div class="fw-bold">${newsData.title}</div>
                        <small class="text-muted">${new Date(newsData.createdDate).toLocaleString('vi-VN')}</small>
                    </div>
                    <span class="badge bg-${newsData.status ? 'success' : 'secondary'} rounded-pill">
                        ${newsData.status ? 'Active' : 'Inactive'}
                    </span>
                `;
                
                // Add highlight effect
                newNewsItem.style.backgroundColor = '#d4edda';
                newNewsItem.style.borderColor = '#c3e6cb';
                
                // Insert at the top
                listGroup.insertBefore(newNewsItem, listGroup.firstChild);
                
                // Remove highlight after 3 seconds
                setTimeout(() => {
                    newNewsItem.style.backgroundColor = '';
                    newNewsItem.style.borderColor = '';
                }, 3000);
                
                // Remove oldest item if more than 5
                const items = listGroup.querySelectorAll('.news-item');
                if (items.length > 5) {
                    items[items.length - 1].remove();
                }
            }
        }
    }

    updateStaffDashboard(newsData) {
        // Check if it's the current user's news
        const currentUserName = document.querySelector('meta[name="current-user"]')?.content;
        if (newsData.author === currentUserName) {
            // Reload page to update counts and recent news
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        }
    }

    updateNewsIndex(newsData) {
        // Reload page to show new news
        setTimeout(() => {
            window.location.reload();
        }, 2000);
    }

    showNotification(message, type) {
        // Create notification element
        const notification = document.createElement('div');
        notification.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
        notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
        notification.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        
        // Add to body
        document.body.appendChild(notification);
        
        // Auto remove after 5 seconds
        setTimeout(() => {
            if (notification.parentNode) {
                notification.remove();
            }
        }, 5000);
    }

    // Public methods
    async joinGroup(groupName) {
        if (this.isConnected) {
            await this.connection.invoke("JoinGroup", groupName);
        }
    }

    async leaveGroup(groupName) {
        if (this.isConnected) {
            await this.connection.invoke("LeaveGroup", groupName);
        }
    }
}

// Initialize SignalR client when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    window.signalRClient = new SignalRClient();
}); 