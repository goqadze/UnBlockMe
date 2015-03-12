# include <iostream>
# include <cstdio>
# include <algorithm>
# include <cmath>
# include <string>
# include <cstring>
# include <vector>
# include <set>
# include <map>
# include <queue>
# include <cassert>

using namespace std;

const int N = 6;
const long long M = 1e9 + 7;

struct point{
	char x, y;
	point(){}
	point(char x, char y) : x(x), y(y) {}
};

struct plank{
	point first;
	point last;
	plank(){}
	plank(point first, point last) : first(first), last(last) {}
};

struct state{
	vector<plank> planks;
	char board[N][N];
	int from;
	int index;
};

char red = 0;
state oo;
point finish;
vector<state> states;
set<long long> visisted;
queue<state> Q;
vector<point> _planks[20];



long long getHash(state* newState){
	int x = 0;
	long long result = 0LL;
	for (int i = 0; i < N; i++){
		for (int j = 0; j < N; j++){
			result = M * result + newState->board[i][j];
		}
	}
	return result;
}

void enQueue(state* newState, int from){
	long long mask = getHash(newState);
	if (visisted.find(mask) == visisted.end()){
		visisted.insert(mask);
		newState->from = from;
		newState->index = states.size();
		states.push_back(*newState);
		Q.push(*newState);
	}
}

bool checkWin(state* ff){
	int x = ff->planks[red - 1].last.x;
	int y = ff->planks[red - 1].last.y + 1;
	while (y < N && ff->board[x][y] == 0){
		y++;
	}
	return y == N;
}

void win(int index, int d = 0){
	if (index == -1){
		cout << d << endl;
		return;
	}
	win(states[index].from, d + 1);
	for (int i = 0; i < N; i++){
		for (int j = 0; j < N; j++){
			cout << (int)states[index].board[i][j] << " ";
		}
		cout << endl;
	}
	cout << endl;
}

int main() {
#ifdef _DEBUG
	freopen("input.txt", "r", stdin); freopen("output.txt", "w", stdout);
#endif

	for (int i = 0; i < N; i++){
		for (int j = 0; j < N; j++){
			int k; cin >> k;
			oo.board[i][j] = k;
			red = max(red, (char)k);
		}
	}

	int vis = 0;
	for (int i = 0; i < N; i++){
		for (int j = 0; j < N; j++){
			int num = oo.board[i][j];
			if (vis & 1 << num) continue;
			if (num > 0){
				int ii = i, jj = j;
				if (j > 0 && j < N - 1 && (oo.board[ii][jj - 1] == num || oo.board[ii][jj + 1] == num)){
					vis |= 1 << num;
					while (jj > 0 && oo.board[ii][jj - 1] == num)
						jj--;

					while (jj < N && oo.board[ii][jj] == num){
						_planks[num].push_back(point(ii, jj));
						jj++;
					}
				}
				else if (i > 0 && i < N - 1 && (oo.board[ii - 1][jj] == num || oo.board[ii + 1][jj] == num))
				{
					vis |= 1 << num;
					while (ii > 0 && oo.board[ii - 1][jj] == num)
						ii--;
					while (ii < N && oo.board[ii][jj] == num){
						_planks[num].push_back(point(ii, jj));
						ii++;
					}
				}
			}
		}
	}

	for (int i = 0; i < 20; i++){
		if (_planks[i].size() > 0){
			//sort(_planks[i].begin(), _planks[i].end(), [](plank a, plank b) -> bool { return a.first.x < b.first.x; });
			oo.planks.push_back(plank(_planks[i].front(), _planks[i].back()));
		}
	}


	oo.from = -1;
	oo.index = 0;
	states.push_back(oo);

	Q.push(oo);
	visisted.insert(getHash(&oo));
	while (!Q.empty()){
		state ff = Q.front(); Q.pop();

		if (states.size() >= 25000000){
			cout << 0;
			return 0;
		}

		if (checkWin(&ff)){
			/////// oh yeah!!!
			win(ff.index);

			exit(0);
		}

		for (int i = 0; i < red; i++){
			bool horizontal = ff.planks[i].first.x == ff.planks[i].last.x;
			point first = ff.planks[i].first;
			point last = ff.planks[i].last;
			if (horizontal){
				int xx = first.x;
				int yy = last.y;

				// to right
				while (yy < N - 1){
					yy++;
					if (ff.board[xx][yy] == 0){
						state newState = ff;
						newState.planks[i].first = point(first.x, first.y + yy - last.y);
						newState.planks[i].last = point(last.x, yy);
						for (int ty = first.y; ty <= last.y; ty++)
							newState.board[first.x][ty] = 0;
						for (int ty = first.y + yy - last.y; ty <= yy; ty++)
							newState.board[first.x][ty] = i + 1;

						enQueue(&newState, ff.index);
					}
					else
						break;
				}

				yy = first.y;
				// to left
				while (yy > 0){
					yy--;
					if (ff.board[xx][yy] == 0){
						state newState = ff;
						newState.planks[i].first = point(first.x, yy);
						newState.planks[i].last = point(last.x, last.y - (first.y - yy));
						for (int ty = first.y; ty <= last.y; ty++)
							newState.board[first.x][ty] = 0;
						for (int ty = yy; ty <= last.y - (first.y - yy); ty++)
							newState.board[first.x][ty] = i + 1;

						enQueue(&newState, ff.index);
					}
					else
						break;
				}

			}
			else
			{
				int xx = first.x;
				int yy = first.y;
				// to up
				while (xx > 0){
					xx--;
					if (ff.board[xx][yy] == 0){
						state newState = ff;
						newState.planks[i].first = point(xx, first.y);
						newState.planks[i].last = point(last.x - (first.x - xx), last.y);
						for (int tx = first.x; tx <= last.x; tx++)
							newState.board[tx][first.y] = 0;
						for (int tx = xx; tx <= last.x - (first.x - xx); tx++)
							newState.board[tx][first.y] = i + 1;

						enQueue(&newState, ff.index);
					}
					else
						break;
				}

				xx = last.x;
				// to down
				while (xx < N - 1){
					xx++;
					if (ff.board[xx][yy] == 0){
						state newState = ff;
						newState.planks[i].first = point(first.x + (xx - last.x), first.y);
						newState.planks[i].last = point(xx, last.y);
						for (int tx = first.x; tx <= last.x; tx++)
							newState.board[tx][first.y] = 0;
						for (int tx = first.x + (xx - last.x); tx <= xx; tx++)
							newState.board[tx][first.y] = i + 1;

						enQueue(&newState, ff.index);
					}
					else
						break;
				}

			}
		}
	}

	cout << 0;
	return 0;
}